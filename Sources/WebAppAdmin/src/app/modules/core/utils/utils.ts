import {MasterAccountGridVm} from '../../ui-admin/pages/master-account/master-account-grid.vm';

export class Utils {
  static resolveMasterAccountName(masterAccount: MasterAccountGridVm): string {
    return (typeof masterAccount.accountAlias !== 'undefined' && masterAccount.accountAlias)
      ? masterAccount.accountAlias : masterAccount.accountName;
  }

  static isNullOrUndefined(obj: any): boolean {
    return typeof obj === 'undefined' || obj == null;
  }
}

