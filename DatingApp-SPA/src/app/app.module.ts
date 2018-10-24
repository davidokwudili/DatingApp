import { BrowserModule } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { HttpClientModule } from "@angular/common/http";

import { AppComponent } from "./app.component";
import { ValueComponent } from "./components/value/value.component";
import { NavComponent } from "./components/nav/nav.component";

import { AuthService } from "./services/auth.service";

@NgModule({
  declarations: [AppComponent, ValueComponent, NavComponent],
  imports: [BrowserModule, HttpClientModule],
  providers: [AuthService],
  bootstrap: [AppComponent]
})
export class AppModule {}
