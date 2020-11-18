import { Label } from 'ng2-charts';
import { IEntityModel } from './i-entity.model';


export class ChartDataModel implements IEntityModel {
  id: number;
  total: number[];
  // expression: string;
  data: UnitData[];
  labels: Label[];
  static GetDefaultChartDataModel(): ChartDataModel {
    return { id: 0, total: [], labels: [], data: [{ data: [], label: '' }] };
  }
}

class UnitData {
  data: number[];
  label: string;
}
