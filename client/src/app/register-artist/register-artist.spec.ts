import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterArtist } from './register-artist';

describe('RegisterArtist', () => {
  let component: RegisterArtist;
  let fixture: ComponentFixture<RegisterArtist>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterArtist],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterArtist);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
