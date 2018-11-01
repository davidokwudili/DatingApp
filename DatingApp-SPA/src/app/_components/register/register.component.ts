import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // tslint:disable-next-line:no-output-on-prefix
  @Output()
  onCancelRegister = new EventEmitter();

  model: any = {};

  constructor(private authSrv: AuthService, private alert: AlertifyService) {}

  ngOnInit() {}

  onRegister() {
    this.authSrv.register(this.model).subscribe(
      _next => {
        this.alert.success('Registered successfully.');
        // console.log("register success");
      },
      _error => {
        console.log(_error);
        this.alert.error(_error);
      }
    );
  }

  onCalcel() {
    this.onCancelRegister.emit();
  }
}
