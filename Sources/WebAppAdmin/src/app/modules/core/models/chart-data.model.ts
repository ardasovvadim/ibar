export class ChartDataModel {
  labels: string[];
  data: DataSet[];
}

export function getDefaultChartDataModel(): ChartDataModel {
  return {labels: [], data: []};
}

export class DataSet {
  data: number[];
  label: string;
}
