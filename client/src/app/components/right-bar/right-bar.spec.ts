import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RightBar } from './right-bar';

describe('RightBar', () => {
  let component: RightBar;
  let fixture: ComponentFixture<RightBar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RightBar],
    }).compileComponents();

    fixture = TestBed.createComponent(RightBar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
