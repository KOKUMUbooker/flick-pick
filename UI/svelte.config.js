import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({
			pages: './build',
			assets: './build',
			fallback: 'index.html',
			precompress: false,
			strict: false
		}),
		alias: {
			'@/*': './src/lib/*'
		},
		paths: {
			base: '' // <-- ensure base path is empty
		}
	}
};

export default config;
