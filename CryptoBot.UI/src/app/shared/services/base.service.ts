import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { UrlQueryStringBuilder } from '../utils';
import { ApiError } from '../model';

@Injectable()
export class BaseService<T, S> {

    constructor(public _http: Http, public _url: string) { }

  handleError (error: Response | any) {
    // In a real world app, we might use a remote logging infrastructure
    let errMsg: string;
    if (error instanceof Response) {
      const err = error['_body'] || JSON.stringify(error);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }

    console.error(errMsg);

    const apiError = <ApiError>error._body;
    error.message = apiError;
    return Observable.throw(error);
  }

    search(requestModel: any): Observable<S> {

      let requestUrl = '';

        if(requestModel !== undefined && requestModel !== null) {
          if(isNaN(requestModel)) {
            requestUrl = UrlQueryStringBuilder.build(this._url + '/search', requestModel);
          }

        }
        return this._http.get(requestUrl)
        .map((response: Response) => <S> response.json())
        .catch(this.handleError);
    }

    get(requestModel: any): Observable<T> {

        let requestUrl = '';

        if(requestModel !== undefined && requestModel !== null) {
          if(isNaN(requestModel)) {
            requestUrl = UrlQueryStringBuilder.build(this._url, requestModel);
          } else {
            requestUrl = this._url + '/' + requestModel;
          }

        }

         return this._http.get(requestUrl)
        .map((response: Response) => <T> response.json())
        .catch(this.handleError);
    }

    put(requestModel: any): Observable<T> {

         return this._http.put(this._url,  requestModel )
        .map((response: Response) => <T> response.json())
        .catch(this.handleError);
    }

    post(requestModel: any): Observable<T> {

         return this._http.post(this._url,  requestModel)
        .map((response: Response) => <T> response.json())
        .catch(this.handleError);
    }

    delete(recordId: number): Observable<T> {

         return this._http.delete(`${this._url}/${recordId}` )
        .map((response: Response) => <T> response.json())
        .catch(this.handleError);
    }
}
