import { APPCONFIG } from './../../config';
import { Component, AfterViewInit, Input } from '@angular/core';
import { OnInit } from '@angular/core/src/metadata/lifecycle_hooks';
import * as $ from 'jquery';

declare const TradingView: any;
declare const Datafeeds: any;
@Component({
    selector: 'tradingview',
    templateUrl: 'trading-view.component.html',
    styleUrls: ['trading-view.component.scss']
})

export class TradingViewComponent implements AfterViewInit {

    @Input() symbolPair: any = 'BTCEUR';

    ngAfterViewInit() {

        this.setupChart();
    }

  setupChart() {

    const widget = new TradingView.widget({
        fullscreen: false,
        height: '800px',
        width: '100%',
        symbol: this.symbolPair,
        interval: '60',
        container_id: 'tv_chart_container',
        // 	BEWARE: no trailing slash is expected in feed URL
        datafeed: new Datafeeds.UDFCompatibleDatafeed( APPCONFIG.apiUrl + 'Chart', 1000 * 60),
        // datafeed: new Datafeeds.UDFCompatibleDatafeed("https://demo_feed.tradingview.com"),
        library_path: 'charting_library/',
        locale: 'en',
        // 	Regression Trend-related functionality is not implemented yet, so it's hidden for a while
        drawings_access: { type: 'black', tools: [ { name: 'Regression Trend' } ] },
        disabled_features: ['use_localstorage_for_settings'],
        enabled_features: ['study_templates'],
        charts_storage_url: 'http://saveload.tradingview.com',
        charts_storage_api_version: '1.1',
        client_id: 'tradingview.com',
        user_id: 'public_user',
        overrides: {
            'paneProperties.background': '#131722',
            'paneProperties.vertGridProperties.color': '#454545',
            'paneProperties.horzGridProperties.color': '#454545',
            'symbolWatermarkProperties.transparency': 90,
            'scalesProperties.textColor' : '#AAA',
        },
        theme: 'Dark',
        timezone: 'Europe/London',
        debug: true
    });

    // tslint:disable-next-line:only-arrow-functions
    widget.onChartReady(function() {

        // widget.chart().createShape({
        //     time: 1518220800, price: 162, channel: 'open'
        // }, {
        //     shape: 'arrow_up', lock: true
        // });

        widget.chart().createStudy('MACD', false, false, [12, 26, 'close', 9], null, {
            'histogram.color': '#FF006E',
            'histogram.linewidth': 4,
            'macd.color' : '#0094FF',
            'signal.color' : '#FF6A00'
            });
        });
  }
}
