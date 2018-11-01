import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';

import { HomeComponent } from './_components/home/home.component';
import { MemberListComponent } from './_components/member-list/member-list.component';
import { MessagesComponent } from './_components/messages/messages.component';
import { ListsComponent } from './_components/lists/lists.component';
import { RegisterComponent } from './_components/register/register.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  // { path: '**', redirectTo: '', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent },
      { path: 'messages', component: MessagesComponent },
      { path: 'lists', component: ListsComponent }
    ]
  }
];
//   { path: '', redirectTo: 'home', pathMatch: 'full' }

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
