import {MasterAccountGridVm} from '../../master-account/master-account-grid.vm';

export class FtpCredentialModel {
  id: number;
  ftpName: string;
  userName: string;
  userPassword: string;
  url: string;
  masterAccounts: MasterAccountGridVm[];
}
