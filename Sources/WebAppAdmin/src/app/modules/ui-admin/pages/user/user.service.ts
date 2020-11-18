import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Observable } from 'rxjs';
import { UserModel } from './user.model';

@Injectable()
export class UserService {

  private readonly apiUrl = 'admin/user';

  constructor(private api: ApiService) {
  }

  getAllUsers(refresh: boolean = false): Observable<UserModel[]> {
    return this.api.get(`${this.apiUrl}/all`);
  }

  addNewUser(user: UserModel): Observable<any> {
    return this.api.post(`${this.apiUrl}/add`, user);
  }

  deleteAdminUser(user: UserModel): Observable<number> {
    return this.api.delete(`${this.apiUrl}/delete/${user.id}`);
  }

  updateUser(user: UserModel): Observable<number> {
    return this.api.put(`${this.apiUrl}/update`, user);
  }

  changeRole(user: UserModel): Observable<number> {
    return this.api.put(`${this.apiUrl}/update/role`, user);
  }

  resendInvite(id: number): Observable<void> {
    return this.api.put(`${this.apiUrl}/${id}/invite`);
  }
}
