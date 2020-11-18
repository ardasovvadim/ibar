import { TotalAccountListEnum } from './total-account-list.enum';

export class TotalClientsListModel {
  enum: TotalAccountListEnum;
  accountTotalList: Array<{ key: string, value: number }>;
}
