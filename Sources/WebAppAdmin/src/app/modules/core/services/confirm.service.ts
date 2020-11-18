import {Injectable} from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import {ConfirmDialogComponent} from '../../ui-common/components/confirm-dialog/confirm-dialog.component';

@Injectable()
export class ConfirmService {

  constructor(private dialog: MatDialog) {
  }

  public ShowConfirmDialog(message = 'Are you sure?', header = 'Confirm dialog') {
    return this.dialog.open(ConfirmDialogComponent, {data: {header, message}}).afterClosed();
  }

}
