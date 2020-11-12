import { URLSearchParams } from '@angular/http';

/**
 * Builds an url with query string parameters
 */
export class UrlQueryStringBuilder {
  public static build(baseUrl: string = '', ...params: Object[]) {
    const qsParams = new URLSearchParams();

         params.forEach(param => {
        for (const key in param) {
          if (param.hasOwnProperty(key)) {
            const value = param[key];
            if (Array.isArray(value)) {
              value.filter( k => k !== null && k !== undefined)
                .forEach( arrayValue => qsParams.append(key, arrayValue));
            } else if( key !== undefined && value !== undefined ) {
              qsParams.set(key, value);
            }
          }
      }
    });

    const appendChar = qsParams.paramsMap.size === 0  ? '' : baseUrl.indexOf('?') === -1  ? '?' : '&';
    return `${baseUrl}${appendChar}${qsParams.toString()}`;
  }
}
