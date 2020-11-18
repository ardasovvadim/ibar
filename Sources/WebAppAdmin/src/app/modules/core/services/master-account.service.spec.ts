import { TestBed } from '@angular/core/testing';

import { MasterAccountService } from './master-account.service';

describe('MasterAccountService', () => {
  let service: MasterAccountService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MasterAccountService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
