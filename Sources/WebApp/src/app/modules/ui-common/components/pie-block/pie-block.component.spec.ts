import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PieBlockComponent } from './pie-block.component';

describe('PieBlockComponent', () => {
  let component: PieBlockComponent;
  let fixture: ComponentFixture<PieBlockComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PieBlockComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PieBlockComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
