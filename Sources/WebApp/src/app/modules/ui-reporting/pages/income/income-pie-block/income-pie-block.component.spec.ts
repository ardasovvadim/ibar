import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { IncomePieBlockComponent } from './income-pie-block.component';

describe('IncomePieBlockComponent', () => {
  let component: IncomePieBlockComponent;
  let fixture: ComponentFixture<IncomePieBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ IncomePieBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(IncomePieBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
