import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  /**
   *
   */
  constructor(
    private authSrv: AuthService,
    private route: Router,
    private alert: AlertifyService
  ) {}

  canActivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.authSrv.loggedIn()) {
      return true;
    }

    this.alert.error('You cant be faster than your shadow, Login first!!!');
    this.route.navigate(['home']);
    return false;
  }
}
