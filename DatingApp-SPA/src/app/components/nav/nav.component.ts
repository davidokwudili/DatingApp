import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { error } from '@angular/compiler/src/util';
import { AlertifyService } from 'src/app/services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public authSrv: AuthService, private alert: AlertifyService) {}

  ngOnInit() {}

  login() {
    this.authSrv.login(this.model).subscribe(
      _next => {
        this.alert.success('Logged in success.');
      },
      _error => {
        // console.log('login failed');
        this.alert.error(_error);
      }
    );
  }

  isLoggedIn() {
    // get the token from local storage
    const token = localStorage.getItem('token');
    // return true if has value n false if not
    return !!token;
    // this.authSrv.loggedIn();
  }

  LogOut() {
    localStorage.removeItem('token');
    this.alert.message('Logged out.');
  }
}
