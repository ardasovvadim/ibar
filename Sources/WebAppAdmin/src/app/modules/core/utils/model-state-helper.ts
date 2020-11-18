export class ModelStateHelper {
  static modelStateToArrayMessages(modelState: any): string[] {
    const messages = [];
    if (typeof modelState !== 'undefined') {
      Object.keys(modelState).forEach(key => messages.push(...modelState[key]));
    }
    return messages;
  }

  static contains(modelKey: string, modelState: any): boolean {
    if (typeof modelState !== 'undefined') {
      const key = Object.keys(modelState).find(key => key.toLocaleLowerCase().includes(modelKey.toLocaleLowerCase()));
      if (typeof key !== 'undefined') {
        return true;
      }
    }
    return false;
  }

  static getFirstOrEmpty(modelKey, modelState: any): string {
    let result = '';
    const key = Object.keys(modelState).find(key => key.toLocaleLowerCase().includes(modelKey.toLocaleLowerCase()));
    if (typeof key !== 'undefined') {
      result = modelState[key];
    }
    return result;
  }
}
