import { Component } from '@angular/core';
import { LeftBar } from '../components/left-bar/left-bar';
import { CenterBar } from '../components/center-bar/center-bar';
import { RightBar } from '../components/right-bar/right-bar';

@Component({
  selector: 'app-music',
  imports: [LeftBar, CenterBar, RightBar],
  templateUrl: './music.html',
  styleUrl: './music.css',
})
export class Music {}
