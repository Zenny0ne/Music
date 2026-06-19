import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginArtist } from './login-artist';

describe('LoginArtist', () => {
  let component: LoginArtist;
  let fixture: ComponentFixture<LoginArtist>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginArtist],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginArtist);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
