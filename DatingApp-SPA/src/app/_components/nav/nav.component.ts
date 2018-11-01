import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { error } from '@angular/compiler/src/util';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(
    public authSrv: AuthService,
    private alert: AlertifyService,
    private route: Router
  ) {}

  ngOnInit() {}

  login() {
    this.authSrv.login(this.model).subscribe(
      _next => {
        this.alert.success('Logged in success.');
      },
      _error => {
        // console.log('login failed');
        this.alert.error(_error);
      },
      () => {
        // navigate to members page on complete
        this.route.navigate(['members']);
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

    // navigate back to home page
    this.route.navigate(['home']);

    // reset the input feild
    this.model.username = '';
    this.model.password = '';
  }
}
