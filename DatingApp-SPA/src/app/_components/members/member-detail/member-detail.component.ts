import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/User';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import {
  NgxGalleryImage,
  NgxGalleryOptions,
  NgxGalleryAnimation
} from 'ngx-gallery';

import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(
    private userSrv: UserService,
    private authService: AuthService,
    private alert: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
    this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    for (let i = 0; i < this.user.photos.length; i++) {
      imageUrls.push({
        small: this.user.photos[i].url,
        medium: this.user.photos[i].url,
        big: this.user.photos[i].url,
        description: this.user.photos[i].description
      });
    }
    return imageUrls;
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

  sendLike(id: number) {
    this.userSrv.sendLike(this.authService.decodedToken.nameid, id).subscribe(
      data => {
        this.alert.success('You have liked: ' + this.user.knownAs);
      },
      error => {
        this.alert.error(error);
      }
    );
  }
}
