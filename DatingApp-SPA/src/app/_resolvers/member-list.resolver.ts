import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

// Basicaly, helps to get data before activating the Route, which shouldn't cause navigation error
@Injectable()
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 10;

  constructor(
    private userSrv: UserService,
    private alert: AlertifyService,
    private router: Router
  ) {}

  resolve(): Observable<User[]> {
    // get the first user list, without parameters
    return this.userSrv.getUsers(this.pageNumber, this.pageSize).pipe(
      // catch error
      catchError(error => {
        // show error message
        this.alert.error('Problem retrieving data.');
        // redirect back to members pages
        this.router.navigate(['home']);
        // return an observable OF null
        return of(null);
      })
    );
  }
}
