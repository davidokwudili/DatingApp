import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/User';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;

  constructor(
    private userSrv: UserService,
    private alert: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
  }

  // Resolver already solves dis
  // loadSingleUser() {
  //   // cast to number by adding + before the param
  //   this.userSrv.getUser(+this.route.snapshot.params['id']).subscribe(
  //     (_user: User) => {
  //       this.user = _user;
  //     },
  //     _error => {
  //       this.alert.error(_error);
  //     }
  //   );
  // }
}
