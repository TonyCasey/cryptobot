import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';
import { BaseService } from './base.service';
import { RuleResponseDto, RuleSearchResponseDto } from '../model/dto/models';
import { APPCONFIG } from '../../config';

@Injectable()
export class RuleService extends BaseService<RuleResponseDto, RuleSearchResponseDto> {

    constructor(public _http: Http, ) {
        super(_http, APPCONFIG.apiUrl + '/rule');
     }
}