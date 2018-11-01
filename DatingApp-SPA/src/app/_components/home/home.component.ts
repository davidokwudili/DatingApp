import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode: boolean;

  constructor(private route: Router) {}

  ngOnInit() {}

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  onCancelClicked() {
    this.registerMode = !this.registerMode;
  }
}
