import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { PaginatedResult } from '../_models/Pagination';
import { map } from 'rxjs/operators';

// THERE'S NO NEED ANYMORE AS WE USE JWTMODULE IN OUR APP.MODULE THAT HANDLES ALL
// const httpOptions = {
//   headers: new HttpHeaders({
//     Authorization: 'Bearer ' + localStorage.getItem('token')
//   })
// };

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl: any = environment.baseUrl;

  constructor(private http: HttpClient) {}

  getUsers(
    page?,
    itemsPerPage?,
    userParams?,
    likesParam?
  ): Observable<PaginatedResult<User[]>> {
    // a pagination result of type user array
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<
      User[]
    >();

    // the parameter Class
    let params = new HttpParams();
    // add the parameters passed from the user, and null if empty
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('Likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('Likees', 'true');
    }

    return (
      this.http
        // get the users from the url, and pass the parameters set above, which is a reponse type
        .get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
        .pipe(
          map(Response => {
            // get the json result from the body
            paginatedResult.result = Response.body;
            // check if the header from the returned object is not null
            if (Response.headers.get('Pagination') != null) {
              // then get the json data from the header and store the Iagination Interface
              paginatedResult.pagination = JSON.parse(
                Response.headers.get('Pagination')
              );
            }
            // return the result
            return paginatedResult;
          })
        )
    );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + id + '/like/' + recipientId,
      {}
    );
  }
}
