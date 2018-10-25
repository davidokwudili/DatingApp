import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode: boolean;

  constructor() {}

  ngOnInit() {}

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  onCancelClicked() {
    this.registerMode = !this.registerMode;
  }
}
