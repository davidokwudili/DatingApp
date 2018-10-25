import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { error } from '@angular/compiler/src/util';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(private authSrv: AuthService) {}

  ngOnInit() {}

  login() {
    this.authSrv.login(this.model).subscribe(
      _next => {
        console.log('login success');
      },
      _error => {
        console.log('login failed');
      }
    );
  }

  isLoggedIn() {
    // get the token from local storage
    const token = localStorage.getItem('token');
    // return true if has value n false if not
    return !!token;
  }

  LogOut() {
    localStorage.removeItem('token');
  }
}
