import { CoinService } from './../shared/services/coin.service.';
import { BotResponseDto, BotSearchResponseDto,
  ExchangeResponseDto, PositionResponseDto } from './../shared/model/dto/models';
import { Component, OnInit } from '@angular/core';
import { CHARTCONFIG } from '../charts/charts.config';
import { Router } from '@angular/router';
import { BotService, ExchangeService, PositionService } from '../shared/services';
import { Bot, Coin } from '../shared/model';
import { Position } from '../shared/model/position.model';

@Component({
  selector: 'my-dashboard',
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {

  errorMessage: any;
  public bots: Bot[];
  exchanges: ExchangeResponseDto[];
  coins: Coin[];

  constructor(private _router: Router,
    private _botService: BotService,
    private _positionService: PositionService,
    private _exchangeService: ExchangeService,
    private _coinService: CoinService
  ) {

  }

  ngOnInit() {
      this.getBots();
    }

    viewBot(bot: Bot ) {
      this._router.navigate(['app/bot', bot.botId]);
    }


    getBots() {
      this._botService.search({})
      .subscribe((result) => {
        this.bots =  <Bot[]>result.data;
        this.getExchanges();
        this.getCoins();
        this.bots.forEach(bot => {
          this.getPositions(bot);
        })
      }
      ,
        error => this.errorMessage = <any> error
      );
    }

    getPositions(bot: Bot) {
      this._positionService.search({botId: bot.botId})
      .subscribe((result) => {
        bot.positions = <Position[]>result.data;
      },
        error => this.errorMessage = <any> error
      );
    }
    getExchanges() {
      this._exchangeService.search({})
      .subscribe((result) => {
        this.exchanges = result.data
      },
        error => this.errorMessage = <any> error
      );
    }

    getCoins() {
      this._coinService.search({})
      .subscribe((result) => {
        this.coins = result.data;
        this.bots.forEach(bot => {
          bot.baseCoin = this.coins.filter(x => x.coinId === bot.baseCoinId)[0];
          bot.coin = this.coins.filter(x => x.coinId === bot.coinId)[0];
        });
      },
        error => this.errorMessage = <any> error
      );
    }


    getExchange(exchangeId: number) {
      return this.exchanges.find(x => x.exchangeId === exchangeId);
    }

    public netProfit(bot: Bot) {
      let total = 0;
      if(bot.positions) {
      bot.positions.forEach(position => {
          total += position.netProfit;
      });
    }
      return total.toFixed( bot.baseCoin === undefined ? 2 : bot.baseCoin.orderRoundingExponent || 2 );
    }

    getProfitPercent(bot: Bot) {
      let total = 0;
      if(bot.positions) {
      bot.positions.forEach(position => {
          total += position.netProfitPercent;
      });
    }
      return total.toFixed(2);
    }
  }
