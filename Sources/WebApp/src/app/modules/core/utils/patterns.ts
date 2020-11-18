import {FormControl} from '@angular/forms';

export abstract class Patterns {
  static EMAIL_PATTERN = '^([a-zA-Z0-9_\\-\\.]+)@([a-zA-Z0-9_\\-\\.]+)\\.([a-zA-Z]{2,5})$';
  static PASSWORD_PATTERN = '^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*\\W+).{9,15}$';
  static PHONE_PATTERN = '^\\+[0-9]+$';
  static PHONE_CODE_PATTERN = '^[0-9]{6}$';
  static DOMAINS_PATTERN = `sytoss.com;mexem.com`;
  // illegalDomain error
  static ACCOUNT_NAME_PATTERN = '^[A-Za-z]{1}\\d{6,7}$';
  static checkDomain(f: FormControl): any {
    const domains = Patterns.DOMAINS_PATTERN.split(';');
    return !!domains.find(v => f.value.includes(v)) ? null : { illegalDomain: true };
  }
}

