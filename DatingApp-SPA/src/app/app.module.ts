import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { BsDropdownModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { ValueComponent } from './_components/value/value.component';
import { NavComponent } from './_components/nav/nav.component';
import { HomeComponent } from './_components/home/home.component';
import { RegisterComponent } from './_components/register/register.component';
import { MemberListComponent } from './_components/member-list/member-list.component';
import { ListsComponent } from './_components/lists/lists.component';
import { MessagesComponent } from './_components/messages/messages.component';

import { ErrorIntercepterProvider } from './_services/error.interceptor';

import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';

import { appRoutes, AppRoutingModule } from './Routes';
import { AuthGuard } from './_guards/auth.guard';

@NgModule({
  declarations: [
    AppComponent,
    ValueComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    ListsComponent,
    MessagesComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    BsDropdownModule.forRoot(),
    AppRoutingModule
    // RouterModule.forRoot(appRoutes)
  ],
  providers: [
    AuthService,
    ErrorIntercepterProvider,
    AlertifyService,
    AuthGuard
  ],

  bootstrap: [AppComponent]
})
export class AppModule {}
