import {IEntityModel} from './i-entity.model';

export class MasterAccountModel implements IEntityModel {
  id: number;
  accountName: string;
  accountAlias: string;

  static getDefault(): MasterAccountModel {
    return {
      id: -1,
      accountName: 'All master accounts',
      accountAlias: 'All master accounts'
    };
  }
}
