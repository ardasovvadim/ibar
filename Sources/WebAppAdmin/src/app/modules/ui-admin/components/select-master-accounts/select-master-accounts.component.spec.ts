import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectMasterAccountsComponent } from './select-master-accounts.component';

describe('SelectMasterAccountsComponent', () => {
  let component: SelectMasterAccountsComponent;
  let fixture: ComponentFixture<SelectMasterAccountsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectMasterAccountsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectMasterAccountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
