import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {MasterAccountModel} from '../../../core/models/master-account.model';
import {MasterAccountService} from '../../../core/services/master-account.service';
import {FormBuilder, FormGroup} from '@angular/forms';
import {FilterService} from '../../../core/services/filter.service';

@Component({
  selector: 'app-account-picker',
  templateUrl: './account-picker.component.html',
  styleUrls: ['./account-picker.component.sass']
})
export class AccountPickerComponent implements OnInit {

  @Output() selectedMasterAccountChange = new EventEmitter<MasterAccountModel>();
  masterAccounts: MasterAccountModel[] = [];
  form: FormGroup;

  constructor(private masterAccountService: MasterAccountService,
              private builder: FormBuilder) {
    this.masterAccounts[0] = MasterAccountModel.getDefault();
  }

  ngOnInit() {
    this.initForm();
    this.masterAccountService.getMasterAccounts().subscribe(data => {
      this.masterAccounts.push(...data);
    });
  }

  setDefault() {
    this.form.controls.selector.setValue(this.masterAccounts[0], {emitEvent: false});
  }

  onChange() {
    const acc = this.form.controls.selector.value;
    this.selectedMasterAccountChange.emit(acc);
  }

  private initForm() {
    this.form = this.builder.group({
      selector: [this.masterAccounts[0]]
    });
  }

  resolveName(masterAccount: MasterAccountModel): string {
    return (typeof masterAccount.accountAlias !== 'undefined' && masterAccount.accountAlias) ? masterAccount.accountAlias : masterAccount.accountName;
  }
}
