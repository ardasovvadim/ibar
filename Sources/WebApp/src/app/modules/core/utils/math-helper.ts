export class MathHelper {
  static Round(val: number): number {
    const str = val.toString();
    const position = str.indexOf('.');
    if (position > 0) {
      return +str.substr(0, position);
    } else {
      return val;
    }
  }
}
