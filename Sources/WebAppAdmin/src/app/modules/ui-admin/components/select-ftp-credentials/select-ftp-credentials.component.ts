import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {IdNameModel} from '../../../core/models/id-name.model';
import {FtpCredentialService} from '../../pages/settings/ftp-credential/ftp-credential.service';
import {Utils} from '../../../core/utils/utils';

@Component({
  selector: 'app-select-ftp-credentials',
  templateUrl: './select-ftp-credentials.component.html',
  styleUrls: ['./select-ftp-credentials.component.sass']
})
export class SelectFtpCredentialsComponent implements OnInit {

  @Output() selectedChange = new EventEmitter<IdNameModel>();
  ftpCredentials: IdNameModel[] = [{id: 0, name: 'All'}];
  selectedFtpCredential: IdNameModel = this.ftpCredentials[0];
  @Input() value = 0;

  constructor(private ftpCredService: FtpCredentialService) {
  }

  ngOnInit() {
    this.ftpCredService.getIdNames().subscribe(data => {
      this.ftpCredentials.push(...data);
      const cred = this.ftpCredentials.find(c => c.id === this.value);
      if (!Utils.isNullOrUndefined(cred)) {
        this.selectedFtpCredential = cred;
      }
    });
  }

  onSelectedChange() {
    this.selectedChange.emit(this.selectedFtpCredential);
  }

}
