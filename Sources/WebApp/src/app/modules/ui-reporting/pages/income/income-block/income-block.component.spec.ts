import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncomeBlockComponent } from './income-block.component';

describe('IncomeBlockComponent', () => {
  let component: IncomeBlockComponent;
  let fixture: ComponentFixture<IncomeBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IncomeBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IncomeBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
