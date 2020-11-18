import { Injectable } from '@angular/core';
import {ApiService} from '../../../core/services/api.service';
import {catchError} from 'rxjs/operators';
import {of} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExceptionService {

  constructor(private  api: ApiService) { }

  getException(excpId :string){
   return  this.api.get('exception/GetException' + excpId).pipe(
     catchError(err => of(null)));
  }
}
