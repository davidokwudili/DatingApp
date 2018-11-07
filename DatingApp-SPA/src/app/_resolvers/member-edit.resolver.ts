import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

// Basicaly, helps to get data before activating the Route, which shouldn't cause navigation error
@Injectable()
export class MemberEditResolver implements Resolve<User> {
  constructor(
    private userSrv: UserService,
    private alert: AlertifyService,
    private authSrv: AuthService,
    private router: Router
  ) {}

  resolve(): Observable<User> {
    // get the id from the routerSnatSh, (It's alread an obersavble, so just use pipe)
    return this.userSrv.getUser(this.authSrv.decodedToken.nameid).pipe(
      // catch error
      catchError(error => {
        // show error message
        this.alert.error('Problem retrieving profile data.');
        // redirect back to members pages
        this.router.navigate(['members']);
        // return an observable OF null
        return of(null);
      })
    );
  }
}
