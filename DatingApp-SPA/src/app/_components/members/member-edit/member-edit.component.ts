import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/User';
import { NgForm } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm')
  editForm: NgForm;
  user: User;

  // Prevent the user from closing the window when editing his/her profile
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alert: AlertifyService,
    private userSrv: UserService,
    private authSrv: AuthService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
  }

  updateUser() {
    this.userSrv
      .updateUser(this.authSrv.decodedToken.nameid, this.user)
      .subscribe(
        next => {
          this.alert.success('Saved Successfully');
          // reset it to our user
          this.editForm.reset(this.user);
        },
        error => {
          this.alert.error(error);
        }
      );
  }
}
