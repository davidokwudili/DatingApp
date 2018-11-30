import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from '../_models/Pagination';
import { Message } from '../_models/Message';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl: any = environment.baseUrl;

  constructor(private http: HttpClient) {}

  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
    // a pagination result of type Message array
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<
      Message[]
    >();

    let params = new HttpParams();
    // add the parameters passed from the user, and null if empty, would use default from the api
    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return (
      this.http
        // get the messages from the url, and pass the parameters set above, which is a reponse type
        .get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {
          observe: 'response',
          params
        })
        .pipe(
          map(response => {
            // get the json result from the body
            paginatedResult.result = response.body;
            // check if the header from the returned object is not null
            if (response.headers.get('Pagination') !== null) {
              // then get the json data from the header and store the Iagination Interface
              paginatedResult.pagination = JSON.parse(
                response.headers.get('Pagination')
              );
            }
            // return the result
            return paginatedResult;
          })
        )
    );
  }

  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(
      this.baseUrl + 'users/' + id + '/messages/thread/' + recipientId
    );
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/messages/' + id,
      {}
    );
  }

  markAsRead(userId: number, messageId: number) {
    this.http
      .post(
        this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read',
        {}
      )
      .subscribe();
  }
}
