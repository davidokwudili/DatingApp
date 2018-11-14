import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder
} from '@angular/forms';
import { User } from 'src/app/_models/User';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // tslint:disable-next-line:no-output-on-prefix
  @Output()
  onCancelRegister = new EventEmitter();
  registerForm: FormGroup;
  user: User;

  constructor(
    private authSrv: AuthService,
    private router: Router,
    private alert: AlertifyService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {
    this.createRegisterForm();
    // this.registerForm = new FormGroup(
    //   {
    //     username: new FormControl('', Validators.required),
    //     password: new FormControl('', [
    //       Validators.required,
    //       Validators.minLength(4),
    //       Validators.maxLength(10)
    //     ]),
    //     confirmPassword: new FormControl('', Validators.required)
    //   },
    //   this.passwordMatchValidator
    // );
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(4),
            Validators.maxLength(8)
          ]
        ],
        confirmPassword: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  onRegister() {
    if (this.registerForm.valid) {
      // get the values from the form and assign to our user model
      this.user = Object.assign({}, this.registerForm.value);
      // send to server
      this.authSrv.register(this.user).subscribe(
        _next => {
          this.alert.success('Registered successfully.');
          // console.log("register success");
        },
        _error => {
          // console.log(_error);
          this.alert.error(_error);
        },
        () => {
          // on completed, save the registered user and log him/her in
          this.authSrv.login(this.user).subscribe(() => {
            // redirect to members page
            this.router.navigate(['members']);
          });
        }
      );
    }
  }

  onCalcel() {
    this.onCancelRegister.emit();
  }
}
