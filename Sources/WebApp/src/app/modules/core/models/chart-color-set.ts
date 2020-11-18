export class Colors {
  static BLACK_COLORS: string[] = [
    '#434A53',
    '#656D77',
    '#AAB2BC',
    '#CCD1D8',
    '#E6E9ED',
  ];
  static COLORS: string[] = [
    '#e58e26',
    '#b71540',
    '#0c2461',
    '#0a3d62',
    '#079992',

    '#fa983a',
    '#eb2f06',
    '#1e3799',
    '#3c6382',
    '#38ada9',

    '#f6b93b',
    '#e55039',
    '#4a69bd',
    '#60a3bc',
    '#78e08f',

    '#fad390',
    '#f8c291',
    '#6a89cc',
    '#82ccdd',
    '#b8e994'
  ];
  static MATERIAL: string[] = [
    '#B71C1C',
    '#AD1457',
    '#4A148C',
    '#4527A0',
    '#1A237E',
    '#0D47A1',
    '#1565C0',
    '#01579B',
    '#006064',
    '#006064',
    '#004D40',
    '#1B5E20',
    '#33691E',
    '#827717',
    '#F57F17',
    '#FF6F00',
    '#E65100',
    '#BF360C',
    '#3E2723',
    '#212121',
    '#263238',
  ];
  static getColor(index: number, colorSet: number): string {
    return Colors.COLORS[(index + (5 * colorSet)) % Colors.COLORS.length];
  }

  static getColorSet(index: number) {
    return Colors.COLORS.slice(index * 5, (index * 5) + 5);
  }
}



