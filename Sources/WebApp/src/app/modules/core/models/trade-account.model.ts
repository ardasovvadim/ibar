import {IEntityModel} from './i-entity.model';

export class TradeAccountModel implements IEntityModel {
  id: number;
  accountName: string;
  accountAlias: string;
  name: string;
  dateFunded: Date;
  mobile: string;
  city: string;

  static resolveName(acc: TradeAccountModel) {
    return (typeof acc.name !== 'undefined' && acc.name !== null)
      ? acc.name
      : (typeof acc.accountAlias !== 'undefined' && acc.accountAlias !== null)
        ? acc.accountAlias
        : acc.accountName;
  }
}




