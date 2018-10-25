import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

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

  constructor(private authSrv: AuthService) {}

  ngOnInit() {}

  onRegister() {
    this.authSrv.register(this.model).subscribe(
      _next => {
        console.log('register success');
      },
      _error => {
        console.log('register failed');
      }
    );
  }

  onCalcel() {
    this.onCancelRegister.emit();
  }
}
