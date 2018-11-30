import { Injectable } from '@angular/core';
import { User } from '../_models/User';
import { Resolve, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MessageService } from '../_services/message.service';
import { AuthService } from '../_services/auth.service';

// Basicaly, helps to get data before activating the Route, which shouldn't cause navigation error
@Injectable()
export class MessagesResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 10;
  messageContainer = 'Unread';

  constructor(
    private mssgSrv: MessageService,
    private authSrv: AuthService,
    private alert: AlertifyService,
    private router: Router
  ) {}

  resolve(): Observable<User[]> {
    // get the first user list, without parameters
    return this.mssgSrv
      .getMessages(
        this.authSrv.decodedToken.nameid,
        this.pageNumber,
        this.pageSize,
        this.messageContainer
      )
      .pipe(
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
