import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IdNameModel} from '../../../core/models/id-name.model';
import {MasterAccountService} from '../../../core/services/master-account.service';
import {Utils} from '../../../core/utils/utils';

@Component({
  selector: 'app-select-master-accounts',
  templateUrl: './select-master-accounts.component.html',
  styleUrls: ['./select-master-accounts.component.sass']
})
export class SelectMasterAccountsComponent implements OnInit {

  @Output() selectedChange = new EventEmitter<IdNameModel>();
  masterAccounts: IdNameModel[] = [{id: 0, name: 'All'}];
  selectedMasterAccount: IdNameModel = this.masterAccounts[0];
  @Input() value = 0;

  constructor(private masterAccountService: MasterAccountService) {
  }

  ngOnInit() {
    this.masterAccountService.getMasterAccountIdNamesModel().subscribe(data => {
      this.masterAccounts.push(...data);
      const master = this.masterAccounts.find(acc => acc.id === this.value);
      if (!Utils.isNullOrUndefined(master)) {
        this.selectedMasterAccount = master;
      }
    });
  }

  onSelectedChange() {
    this.selectedChange.emit(this.selectedMasterAccount);
  }
}
