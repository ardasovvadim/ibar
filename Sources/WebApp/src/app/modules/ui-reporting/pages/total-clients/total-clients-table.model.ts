import {IEntityModel} from '../../../core/models/i-entity.model';

export class TotalClientsTableModel implements IEntityModel {
  id: number;
  rowName: string;
  lastDay: string;
  mtd: string;
  lastMonth: string;
  avgDailyMonth: string;
  avgDailyYear: string;
}
