import {Component, OnInit, ViewChild} from '@angular/core';
import { MatCheckbox, MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import {UserService} from './user.service';
import {UserModel} from './user.model';
import {UserAddDialogComponent} from './user-add-dialog/user-add-dialog.component';
import {UserEditDialogComponent} from './user-edit-dialog/user-edit-dialog.component';
import {LoadingService} from '../../../core/services/loading.service';
import {finalize} from 'rxjs/operators';
import {ConfirmService} from '../../../core/services/confirm.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.sass']
})
export class UserComponent implements OnInit {

  displayedColumns: string[] = ['position', 'name', 'email', 'phone', 'isAdmin', 'isWaitingConfirmation', 'tools'];
  users: UserModel[];
  @ViewChild(MatTable, {static: true}) table: MatTable<any>;

  constructor(private userService: UserService,
              private dialog: MatDialog,
              private loading: LoadingService,
              private confirm: ConfirmService) {
  }

  ngOnInit() {
    this.loading.setLoading(true);
    this.userService.getAllUsers().pipe(
      finalize(() => this.loading.setLoading(false))
    ).subscribe(
      data => {
        this.users = data;
      });
  }

  addUser() {
    this.dialog.open(UserAddDialogComponent)
      .afterClosed()
      .subscribe(() => this.refreshData());
  }

  deleteUser(user: UserModel) {
    this.confirm.ShowConfirmDialog().subscribe(answer => {
      if (answer) {
        this.loading.setLoading(true);
        this.userService.deleteAdminUser(user).pipe(
          finalize(() => this.loading.setLoading(false))
        ).subscribe(() => this.refreshData());
      }
    });
  }

  getNameUser(user: UserModel) {
    const f = user.firstName ? user.firstName : '';
    const l = user.lastName ? user.lastName : '';
    return f + ' ' + l;
  }

  editUser(user: UserModel) {
    this.dialog.open(UserEditDialogComponent, {data: {...user}})
      .afterClosed()
      .subscribe(() => this.refreshData());
  }

  refreshData() {
    this.loading.setLoading(true);
    this.userService.getAllUsers(true).pipe(
      finalize(() => this.loading.setLoading(false))
    ).subscribe(
      data => {
        this.users = data;
      },
      () => this.users = []);
  }

  isAdmin(user: UserModel): boolean {
    return user.roles.includes('admin');
  }

  onIsAdminChange($event: MatCheckbox, user: UserModel) {
    const value = $event.checked;

    if (value) {
      user.roles.push('admin');
    } else {
      user.roles = user.roles.filter(role => role !== 'admin');
    }

    this.userService.changeRole(user)
      .pipe(finalize(() => this.refreshData()))
      .subscribe();
  }

  discardCheckboxChanges(checkbox: MatCheckboxChange) {
    checkbox.source.checked = !checkbox.checked;
  }

  resendInvite(user: UserModel) {
    this.confirm.ShowConfirmDialog('Are you sure you want to resend the invitation?')
      .subscribe(answer => {
        if (answer) {
          this.userService.resendInvite(user.id).subscribe(() => this.refreshData());
        }
      });
  }
}
