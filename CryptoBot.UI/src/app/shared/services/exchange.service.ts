
import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { BaseService } from './base.service';
import { ExchangeResponseDto, ExchangeSearchResponseDto } from './../model/dto/models';
import { APPCONFIG } from '../../config';

@Injectable()
export class ExchangeService extends BaseService<ExchangeResponseDto, ExchangeSearchResponseDto> {

    constructor(public _http: Http, ) {
        super(_http, APPCONFIG.apiUrl + '/exchange');
     }
}