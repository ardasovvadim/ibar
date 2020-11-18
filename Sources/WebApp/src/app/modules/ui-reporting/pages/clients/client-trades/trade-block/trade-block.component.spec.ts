import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeBlockComponent } from './trade-block.component';

describe('TradeBlockComponent', () => {
  let component: TradeBlockComponent;
  let fixture: ComponentFixture<TradeBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradeBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
