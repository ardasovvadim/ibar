import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectFtpCredentialsComponent } from './select-ftp-credentials.component';

describe('SelectFtpCredentialsComponent', () => {
  let component: SelectFtpCredentialsComponent;
  let fixture: ComponentFixture<SelectFtpCredentialsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectFtpCredentialsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectFtpCredentialsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
