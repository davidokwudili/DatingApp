import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/User';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];

  constructor(
    private userSrv: UserService,
    private alert: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.users = data['users'];
    });
  }

  // Resolver already solves dis
  // loadUsers() {
  //   this.userSrv.getUsers().subscribe(
  //     (_users: User[]) => {
  //       this.users = _users;
  //     },
  //     error => {
  //       this.alert.error(error);
  //     }
  //   );
  // }
}
